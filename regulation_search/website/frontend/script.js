class ChatInterface {
    constructor() {
        this.messagesContainer = document.getElementById('chatMessages');
        this.messageInput = document.getElementById('messageInput');
        this.sendButton = document.getElementById('sendButton');
        this.loadingIndicator = document.getElementById('loadingIndicator');
        
        this.isProcessing = false;
        this.threadId = null; // Store thread_id for conversation continuity
        
        this.initializeEventListeners();
        this.setupTextareaAutoResize();
    }
    
    initializeEventListeners() {
        // Send button click
        this.sendButton.addEventListener('click', () => this.handleSendMessage());
        
        // Enter key to send (Shift+Enter for new line)
        this.messageInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.handleSendMessage();
            }
        });
        
        // Input event for button state
        this.messageInput.addEventListener('input', () => {
            this.updateSendButtonState();
        });
        
        // Focus input on load
        this.messageInput.focus();
    }
    
    setupTextareaAutoResize() {
        this.messageInput.addEventListener('input', () => {
            // Reset height to auto to get the correct scrollHeight
            this.messageInput.style.height = 'auto';
            
            // Set the height based on scrollHeight
            const maxHeight = 120; // max-height from CSS
            const newHeight = Math.min(this.messageInput.scrollHeight, maxHeight);
            this.messageInput.style.height = newHeight + 'px';
        });
    }
    
    updateSendButtonState() {
        const hasText = this.messageInput.value.trim().length > 0;
        this.sendButton.disabled = !hasText || this.isProcessing;
    }
    
    async handleSendMessage() {
        const message = this.messageInput.value.trim();
        
        if (!message || this.isProcessing) {
            return;
        }
        
        // Add user message to chat
        this.addMessage(message, 'user');
        
        // Clear input and reset height
        this.messageInput.value = '';
        this.messageInput.style.height = 'auto';
        
        // Set processing state
        this.setProcessingState(true);
        
        try {
            // Send message to backend
            const response = await this.sendToBackend(message);
            
            // Add assistant response
            this.addMessage(response, 'assistant');
            
        } catch (error) {
            console.error('Error sending message:', error);
            this.addMessage(
                'I apologize, but I encountered an error while processing your request. Please try again.',
                'assistant',
                true // isError
            );
        } finally {
            this.setProcessingState(false);
        }
    }
    
    // Method to clear conversation and reset thread
    clearConversation() {
        this.messagesContainer.innerHTML = '';
        this.threadId = null;
        console.log('Conversation cleared, thread_id reset');
    }
    
    async sendToBackend(message) {
        // Prepare request body
        const requestBody = {
            query: message
        };
        
        // Include thread_id if we have one (not for the first message)
        if (this.threadId) {
            requestBody.thread_id = this.threadId;
            console.log('Sending with thread_id:', this.threadId);
        } else {
            console.log('First message, no thread_id sent');
        }
        
        const response = await fetch('https://func-single-agent-demo-eus2-mx01.azurewebsites.net/api/send', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestBody)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        console.log('Response received:', data);
        
        // Store thread_id for subsequent requests
        if (data.thread_id) {
            this.threadId = data.threadId;
            console.log('Thread ID stored:', this.threadId);
        }
        
        // Return the response field content
        return data.response || 'No response received';
    }
    
    addMessage(content, sender, isError = false) {
        const messageWrapper = document.createElement('div');
        messageWrapper.className = `message-wrapper ${sender} new`;
        
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${sender}-message${isError ? ' error' : ''}`;
        
        const messageContent = document.createElement('div');
        messageContent.className = 'message-content';
        
        if (sender === 'assistant') {
            // Render markdown for assistant messages
            messageContent.innerHTML = marked.parse(content);
        } else {
            // Plain text for user messages
            messageContent.textContent = content;
        }
        
        const messageTime = document.createElement('div');
        messageTime.className = 'message-time';
        messageTime.textContent = this.formatTime(new Date());
        
        messageDiv.appendChild(messageContent);
        messageDiv.appendChild(messageTime);
        messageWrapper.appendChild(messageDiv);
        
        this.messagesContainer.appendChild(messageWrapper);
        
        // Scroll to bottom
        this.scrollToBottom();
        
        // Remove animation class after animation completes
        setTimeout(() => {
            messageWrapper.classList.remove('new');
        }, 300);
    }
    
    setProcessingState(isProcessing) {
        this.isProcessing = isProcessing;
        
        if (isProcessing) {
            this.loadingIndicator.classList.add('show');
            this.sendButton.disabled = true;
            this.messageInput.disabled = true;
        } else {
            this.loadingIndicator.classList.remove('show');
            this.messageInput.disabled = false;
            this.updateSendButtonState();
            this.messageInput.focus();
        }
    }
    
    scrollToBottom() {
        this.messagesContainer.scrollTop = this.messagesContainer.scrollHeight;
    }
    
    formatTime(date) {
        const now = new Date();
        const isToday = date.toDateString() === now.toDateString();
        
        if (isToday) {
            return date.toLocaleTimeString([], { 
                hour: '2-digit', 
                minute: '2-digit' 
            });
        } else {
            return date.toLocaleDateString([], { 
                month: 'short', 
                day: 'numeric',
                hour: '2-digit', 
                minute: '2-digit' 
            });
        }
    }
}

// Error handling for marked.js
if (typeof marked === 'undefined') {
    console.warn('Marked.js not loaded, falling back to plain text rendering');
    window.marked = {
        parse: function(text) {
            return text.replace(/\n/g, '<br>');
        }
    };
}

// Initialize the chat interface when the DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new ChatInterface();
});

// Global error handler for unhandled promises
window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled promise rejection:', event.reason);
    event.preventDefault();
});

// Export for potential testing or external access
window.ChatInterface = ChatInterface;
