import React, { Component } from 'react';
import { connect } from 'react-redux';
import Checkbox from '@hig/checkbox';
import { checkedProjects } from '../reducers/mainReducer';

export class CheckboxTableHeader extends Component {

  onChange(checked) {
    const { allRowsCount, checkedProjects } = this.props;
    const clearAll = !checked && checkedProjects.length === allRowsCount;
    this.props.onChange( clearAll );
  }

  render() {
    const { allRowsCount, selectable, checkedProjects } = this.props;
    const hasSomeCheckedProject = checkedProjects.length > 0;
    const selectedAll = checkedProjects.length === allRowsCount;
    const indeterminate = hasSomeCheckedProject && !selectedAll;
    const visible = selectable && allRowsCount > 0;

      return (
        <div>
          {visible && <Checkbox
            indeterminate={indeterminate}
            checked={hasSomeCheckedProject}
            onChange={(checked) => {this.onChange(checked); }}
          />}
        </div>
        );
    }
  }

export default connect(function (store) {
  return {
    checkedProjects: checkedProjects(store)
  };
})(CheckboxTableHeader);