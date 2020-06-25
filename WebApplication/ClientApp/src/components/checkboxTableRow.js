import React, { Component } from 'react';
import { connect } from 'react-redux';
import Checkbox from '@hig/checkbox';
import { checkedProjects } from '../reducers/mainReducer';

export class CheckboxTableRow extends Component {

  onChange(checked) {
    const { rowData } = this.props;
    this.props.onChange( checked, rowData );
  }

  render() {
    const { rowData, selectable } = this.props;
    const isChecked = this.props.checkedProjects.includes(rowData.id);

      return (
        <div>
          {selectable && <Checkbox
            onChange={(checked) => {this.onChange(checked); }}
            checked={isChecked}
          />}
        </div>
        );
    }
  }

export default connect(function (store) {
  return {
    checkedProjects: checkedProjects(store)
  };
})(CheckboxTableRow);