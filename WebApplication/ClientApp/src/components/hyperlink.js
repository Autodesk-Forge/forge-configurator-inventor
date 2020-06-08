import React, { Component } from 'react';

export class HyperLink extends Component {

    constructor(){
        super();
    }

    componentDidMount() {
        if (this.props.onAutostart)
            this.props.onAutostart(this.downloadHyperlink);
    }

    render() {
        const downloadLink = <a href={this.props.href} onClick={(e) => {
            e.stopPropagation();
            if (this.props.onUrlClick) this.props.onUrlClick();
        }} ref = {(h) => {
            this.downloadHyperlink = h;
        }}>{this.props.link}</a>;

      return(
          <div style={{ width: 'fit-content'}}>
            {this.props.prefix}{downloadLink}{this.props.suffix}
          </div>
      );
    }
  }

  export default HyperLink;