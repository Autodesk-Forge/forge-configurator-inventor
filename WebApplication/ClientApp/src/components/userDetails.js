/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import React, { Component } from 'react';
import Avatar from '@hig/avatar';
import "./userDetails.css";
import Button from '@hig/button';

export class UserDetails extends Component {
    constructor(props) {
        super(props);
        this.handleAuthClick = this.handleAuthClick.bind(this);
    }

    handleAuthClick() {
        if (this.props.profile.isLoggedIn) {
            this.props.logout();
        } else {
            window.location.href = '/login';
        }
    }

    render() {
        return (
            <div>
                <div>
                    <span className="user">USER</span>
                    <Avatar className={"avatar-custom-style"} size="large" name={this.props.profile.name} image={this.props.profile.avatarUrl} />
                </div>
                <span className="username">{this.props.profile.name}</span>
                <Button className="auth-button" style={
                            { width: '244px', height: '36px', borderRadius: '2px', border: '1px solid rgb(128, 128, 128)', margin: '12px'}}
                            type="secondary"
                            size="small"
                            title={this.props.profile.isLoggedIn ? "Sign Out" : "Sign In"}
                            onClick={this.handleAuthClick}
                        />
                <span className="hyperlink">
                    <a href="https://github.com/Developer-Autodesk/forge-configurator-inventor/blob/master/about.md">About application</a>
                </span>
            </div>
        );
    }
}

export default UserDetails;