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
            window.location.reload(false);
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
            </div>
        );
    }
}

export default UserDetails;