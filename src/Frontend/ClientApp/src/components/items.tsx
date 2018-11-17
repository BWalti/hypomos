import {identity} from '../utils/'

export const menuItems = [{
  ariaLabel: 'Selection Mode',
  items: [{
    canCheck: true,
    checked: false,
    key: 'selectionModeNone',
    name: 'None',
  }, {
    canCheck: true,
    checked: true,
    key: 'selectionModeSingle',
    name: 'Single Mode',
  }, {
    canCheck: true,
    checked: false,
    key: 'selectionModeMulti',
    name: 'Multi Mode',
  }],
  key: 'newItem',
  name: 'Selection Mode',
  onClick: identity,
}, {
  icon: 'Upload',
  key: 'upload',
  name: 'Upload',
  onClick: identity,
}, {
  icon: 'Share',
  key: 'share',
  name: 'Share',
  onClick: identity
}, {
  icon: 'Download',
  key: 'download',
  name: 'Download',
  onClick: identity
}, {
  icon: 'MoveToFolder',
  key: 'move',
  name: 'Move to...',
  onClick: identity
}, {
  icon: 'Copy',
  key: 'copy',
  name: 'Copy to...',
  onClick:identity
}, {
  icon: 'Edit',
  key: 'rename',
  name: 'Rename...',
  onClick:identity
}, {
  disabled: true,
  icon: 'Cancel',
  key: 'disabled',
  name: 'Disabled...',
  onClick:identity
}]

export const farMenuItems = [{
  icon: 'SortLines',
  key: 'sort',
  name: 'Sort',
  onClick: identity
}, {
  icon: 'Tiles',
  key: 'tile',
  name: 'Grid view',
  onClick: identity
}, {
  icon: 'Info',
  key: 'info',
  name: 'Info',
  onClick: identity
}]