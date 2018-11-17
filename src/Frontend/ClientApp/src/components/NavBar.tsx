import {SearchBox} from 'office-ui-fabric-react/lib/SearchBox'
import React from 'react'
import '../_styles/NavBar.css'

const NavBar = ({onChange, onSearch}: {onChange: any, onSearch: any}) => (
  <div className="NavBar">
    <div className="logo ms-font-xl">
      <strong>Awesome App</strong>
    </div>
    <div className="searchbox">
      <SearchBox labelText="Search" />
    </div>
  </div>
)

export default NavBar
