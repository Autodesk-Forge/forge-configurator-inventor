import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, FrozenDirection } from 'react-base-table';
import CheckboxTableHeader from './CheckboxTableHeader';
import CheckboxTableRow from './CheckboxTableRow';
import { checkedProjects } from '../reducers/mainReducer';
import { setCheckedProjects, clearCheckedProjects } from '../actions/uiFlagsActions';
import { cloneArray } from 'react-base-table/lib/utils';

const CHECK_COLUMN = 'check_column';

export class CheckboxTable extends Component {

    constructor(props) {
      super(props);

      this.setupCheckColumn();
    }

    onSelectAllChange(clearAll) {
      if (clearAll) {
        this.props.clearCheckedProjects();
      } else {
        const projects = this.props.data.map(item => item.id);
        this.props.setCheckedProjects(projects);
      }
    }

    onSelectChange(checked, rowData) {
      const projects = cloneArray(this.props.checkedProjects);
      const id = rowData.id;
      if (checked)
        projects.push(id);
      else {
        const index = projects.indexOf(id);
        if (index > -1) {
          projects.splice(index, 1);
        }
      }

      this.props.setCheckedProjects(projects);
    }

    setupCheckColumn() {
      const exists = this.props.projectListColumns.find(item => item.key === CHECK_COLUMN);
      if (exists !== undefined)
        return;

      this.props.projectListColumns.unshift({
        width: 38.5,
        flexShrink: 0,
        locked: true,
        resizable: false,
        frozen: FrozenDirection.RIGHT,
        position: 1 - Number.MAX_VALUE,
        headerRenderer: () => (
          <CheckboxTableHeader
            allRowsCount={this.props.data.length}
            onChange={(clearAll) => {this.onSelectAllChange(clearAll); }}
            selectable={this.props.selectable}
          />
        ),
        cellRenderer: ({ rowData }) => (
          <CheckboxTableRow
            rowData={rowData}
            onChange={(checked, rowData) => {this.onSelectChange(checked, rowData); }}
            selectable={this.props.selectable}
          />
        ),
        ...this.props.selectionColumnProps,
        key: CHECK_COLUMN
      });
    }

    render() {

      return (
        <AutoResizer>
          {({ width, height }) => {
                return <BaseTable
                  width={width}
                  height={height}
                  columns={this.props.projectListColumns}
                  data={this.props.data}
                  rowEventHandlers={{
                    onClick: ({ rowData, event }) => {
                      if (event.target.type && event.target.type === "checkbox") {
                        return;
                      }

                      this.props.onProjectClick(rowData.id);
                    }
                  }}
              />;
          }}
        </AutoResizer>
        );
    }
  }

/* istanbul ignore next */
export default connect(function (store) {
  return {
    checkedProjects: checkedProjects(store)
  };
}, { setCheckedProjects, clearCheckedProjects })(CheckboxTable);
