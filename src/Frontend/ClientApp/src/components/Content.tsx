import React from 'react'

import '../styles/Content.css'
// import { farMenuItems as defaultFarMenuItems, menuItems as defaultMenuItems, farMenuItems } from './items.js'

class Content extends React.Component {
    constructor(props: any) {
        super(props)
    }

    public render() {
        return (
            <div className="container">
                <div className="selection" />
            </div>
        )
    }
}

export default Content
