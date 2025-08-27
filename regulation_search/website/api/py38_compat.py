"""py38_compat.py

Python 3.8 compatibility fixes for azure-ai-projects SDK.
This module must be imported before importing azure.ai.projects to patch
compatibility issues with Python 3.8.
"""

import sys
import typing
from typing import Any

def apply_py38_patches():
    """Apply necessary patches for Python 3.8 compatibility."""
    
    if sys.version_info >= (3, 9):
        return  # No patches needed for Python 3.9+
    
    # Patch 1: Fix MutableMapping subscription
    import collections.abc
    if not hasattr(typing, 'MutableMapping'):
        typing.MutableMapping = collections.abc.MutableMapping
    
    # Patch 2: Fix generic alias issues
    try:
        if not hasattr(typing, '_GenericAlias'):
            # Create a simple _GenericAlias for compatibility
            class _GenericAlias:
                def __init__(self, origin, args):
                    self.__origin__ = origin
                    self.__args__ = args
                
                def __getitem__(self, item):
                    return self
                    
                def __repr__(self):
                    return f"{self.__origin__}[{', '.join(str(arg) for arg in self.__args__)}]"
            
            typing._GenericAlias = _GenericAlias
    except Exception:
        pass
    
    # Patch 3: Ensure typing extensions are available
    try:
        import typing_extensions
        # Backport newer typing features for Python 3.8
        if not hasattr(typing, 'get_origin'):
            typing.get_origin = getattr(typing_extensions, 'get_origin', lambda x: None)
        if not hasattr(typing, 'get_args'):
            typing.get_args = getattr(typing_extensions, 'get_args', lambda x: ())
    except ImportError:
        pass
    
    # Patch 4: Fix collections.abc import issues
    try:
        import collections.abc as abc
        
        # Ensure MutableMapping is subscriptable
        original_mutable_mapping = abc.MutableMapping
        
        class SubscriptableMutableMapping(original_mutable_mapping):
            def __class_getitem__(cls, item):
                return cls
        
        abc.MutableMapping = SubscriptableMutableMapping
        typing.MutableMapping = SubscriptableMutableMapping
        
    except Exception:
        pass

# Apply patches immediately when this module is imported
apply_py38_patches()
