import React, { Component } from 'react';
import { connect } from 'react-redux';
import Checkbox from '@hig/checkbox';
import { checkedProjects } from '../reducers/mainReducer';

export class CheckboxTableHeader extends Component {

  onChange(checked) {
    const { checkedProjects } = this.props;
    const allRowsCount = this.props.projectList.projects ? this.props.projectList.projects.length : 0;
    const clearAll = !checked && checkedProjects.length === allRowsCount;
    this.props.onChange( clearAll );
  }

  render() {
    const { selectable, checkedProjects } = this.props;
    const allRowsCount = this.props.projectList.projects ? this.props.projectList.projects.length : 0;
    const hasSomeCheckedProject = checkedProjects.length > 0;
    const selectedAll = checkedProjects.length === allRowsCount;
    const indeterminate = hasSomeCheckedProject && !selectedAll;
    const visible = hasSomeCheckedProject || (selectable && allRowsCount > 0);

      return (
        <div id={hasSomeCheckedProject ? "checkbox_checked_visible" : "checkbox_hover_visible"}>
          {visible && <Checkbox id="checkbox_header"
            indeterminate={indeterminate}
            checked={hasSomeCheckedProject}
            onChange={(checked) => {this.onChange(checked); }}
          />}
        </div>
        );
    }
  }

/* istanbul ignore next */
export default connect(function (store) {
  return {
    projectList: store.projectList,
    checkedProjects: checkedProjects(store)
  };
})(CheckboxTableHeader);