import React from 'react'

import { Breadcrumb, IBreadcrumbItem } from 'office-ui-fabric-react/lib/Breadcrumb'
import { Check } from 'office-ui-fabric-react/lib/Check'
import { CommandBar } from 'office-ui-fabric-react/lib/CommandBar'
import { IContextualMenuItem } from 'office-ui-fabric-react/lib/ContextualMenu'
import { MarqueeSelection } from 'office-ui-fabric-react/lib/MarqueeSelection'
import { Selection, SelectionMode, SelectionZone, } from 'office-ui-fabric-react/lib/utilities/selection'

import '../_styles/Content.css'
import { createListItems } from '../utils/'
// import { farMenuItems as defaultFarMenuItems, menuItems as defaultMenuItems, farMenuItems } from './items.js'

/*Content.propTypes = {
    items: T.arrayOf(T.shape(IBreadcrumbItem)),
    menuItems: T.arrayOf(T.shape(IContextualMenuItem)),
    farMenuItems: T.arrayOf(T.shape(IContextualMenuItem)),
    maxBreadcrumbs: T.number,
}

Content.defaultProps = {
    maxBreadcrumbs: 3,
    breadcrumbs: [
        { text: 'Files', 'key': 'Files', onClick: identity },
        { text: 'This is folder 1', 'key': 'f1', onClick: identity },
        { text: 'This is folder 2', 'key': 'f2', onClick: identity },
        { text: 'This is folder 3', 'key': 'f3', onClick: identity },
        { text: 'This is folder 4', 'key': 'f4', onClick: identity },
        { text: 'Home', 'key': 'f5', onClick: identity },
    ],
    menuItems: defaultMenuItems,
    farMenuItems: defaultFarMenuItems,
}*/

class Content extends React.Component<{
    items: IBreadcrumbItem[],
    menuItems: IContextualMenuItem[],
    farMenuItems: IContextualMenuItem[],
    maxBreadcrumbs: number,
    breadcrumbs: any,
}, {
        canSelect: string,
        items: any,
        selection: any,
        selectionMode: SelectionMode,
    }> {
    private hasMounted: boolean;

    constructor(props: any) {
        super(props)
        this.state = {
            canSelect: 'all',
            items: createListItems(200),
            selection: new Selection({ onSelectionChanged: this.onSelectionChanged }),
            selectionMode: SelectionMode.multiple,
        };

        this.state.selection.setItems(this.state.items, false)
    }

    public componentDidMount() {
        this.hasMounted = true
    }

    public render() {
        const { breadcrumbs, maxBreadcrumbs, menuItems, farMenuItems } = this.props
        const { items, selection, selectionMode } = this.state
        return (
            <div className="container">
                <Breadcrumb className="breadcrumbs" items={breadcrumbs}
                    maxDisplayedItems={maxBreadcrumbs}
                />
                <CommandBar
                    items={menuItems}
                    farItems={farMenuItems}
                />
                <div className="selection">
                    <MarqueeSelection selection={selection} isEnabled={selectionMode === SelectionMode.multiple}>
                        <SelectionZone selection={selection}
                            selectionMode={selectionMode}>
                            {items.map((item: any, index: any) => (
                                <div key={index} className="selection-item" data-selection-index={index}>
                                    {(selectionMode !== SelectionMode.none) && (
                                        <span className="check" data-selection-toggle={true}>
                                            <Check checked={selection.isIndexSelected(index)} />
                                        </span>
                                    )}
                                    <span className="name">{item.name}</span>
                                </div>
                            ))}
                        </SelectionZone>
                    </MarqueeSelection>
                </div>
            </div>
        )
    }

    private onSelectionChanged = () => {
        if (this.hasMounted) { this.forceUpdate() }
    }
}

export default Content
