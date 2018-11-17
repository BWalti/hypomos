import { Nav } from 'office-ui-fabric-react/lib/Nav';
import React from 'react';

const SidebarMenu = (props: any) => {
    const { groups, expanded, collapsed } = props;
    return (
        <div className='SidebarMenu'>
            <Nav groups={groups}
                expandedStateText={expanded}
                collapsedStateText={collapsed}
            />
        </div>
    )
};

SidebarMenu.defaultProps = {
    collapsed: 'collapsed',
    expanded: 'expanded',
    groups: [{
        links: [{
            isExpanded: true,
            links: [{
                name: 'Activity',
                url: 'http://msn.com',
            }, {
                name: 'News',
                url: 'http://msn.com',
            }],
            name: 'Home',
            url: 'http://example.com',
        }, {
            isExpanded: true,
            name: 'Documents',
            url: 'http://example.com',
        }, {
            name: 'Pages',
            url: 'http://msn.com',
        }, {
            name: 'Notebook',
            url: 'http://msn.com',
        }, {
            name: 'Long Name Test for elipsis. Longer than 12em!',
            url: 'http://example.com',
        }, {
            iconClassName: 'ms-Icon--Edit',
            name: 'Edit Link',
            url: 'http://example.com',
        }, {
            icon: 'Edit',
            name: 'Edit',
            onClick: () => { return; },
            url: '#',
        }]
    }],
}

export default SidebarMenu
