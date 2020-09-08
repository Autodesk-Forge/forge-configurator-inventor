import React, { Component } from 'react';
import { connect } from 'react-redux';
import { adoptProjectWithParameters } from '../actions/adoptWithParamsActions';

export class AdoptTest extends Component {
  constructor(props) {
    super(props);
    this.state = { data: '' };

    this.handleChange = this.handleChange.bind(this);
    this.handleAdopt = this.handleAdopt.bind(this);
  }

  handleAdopt() {
      const jsPayload = JSON.parse(this.state.data);
      this.props.adoptProjectWithParameters(jsPayload);
  }

  handleChange(event) {
    this.setState({ data: event.target.value });
  }

  render() {
    return (
      <div>
        <input type="button" value="Adopt" onClick={this.handleAdopt} />
        <textarea
          type="text"
          rows="20"
          cols="100"
          onChange={this.handleChange}
        />
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {};
}, {adoptProjectWithParameters})(AdoptTest);
