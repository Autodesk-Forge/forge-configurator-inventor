import React, { Component } from 'react';
import Avatar from '@hig/avatar';
import "./userDetails.css";

class UserDetails extends Component {
    constructor(props) {
        super(props);
        this.handleAuthClick = this.handleAuthClick.bind(this);
    }

    handleAuthClick() {
        if (this.props.profile.isLoggedIn) {
            const logoutFrame = document.getElementById('hiddenLogoutFrame');
            logoutFrame.onload = () => {
                logoutFrame.removeEventListener('load', this, false);
                window.location.reload(false);
            };
            logoutFrame.src = 'https://accounts.autodesk.com/Authentication/LogOut';
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
                <div className="auth-button">
                    <span className="auth-button-text" onClick={this.handleAuthClick}>
                        {this.props.profile.isLoggedIn ? "Sign Out" : "Sign In"}
                    </span>
                </div>
                {/* Use a hidden iframe to make a logout request to autodesk since it isn't supported in a better way yet */}
                <iframe id="hiddenLogoutFrame" />
            </div>
        );
    }
}

export default UserDetails;