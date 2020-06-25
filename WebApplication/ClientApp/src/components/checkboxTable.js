import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Column } from 'react-base-table';
import BaseTable, { AutoResizer, FrozenDirection } from 'react-base-table';
import CheckboxTableHeader from './checkboxTableHeader';
import CheckboxTableRow from './checkboxTableRow';
import { checkedProjects } from '../reducers/mainReducer';
import { setCheckedProjects, clearCheckedProjects } from '../actions/uiFlagsActions';
import { cloneArray } from 'react-base-table/lib/utils';

const CHECK_COLUMN = 'check_column';
const Icon = ({ iconname }) => (
  <div>
    <img src={iconname} alt='' width='16px' height='18px' />
  </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

export class CheckboxTable extends Component {

    constructor(props) {
      super(props);

      this.projectListColumns = [
        {
          width: 38.5,
          flexShrink: 0,
          locked: true,
          resizable: false,
          frozen: FrozenDirection.RIGHT,
          position: 1 - Number.MAX_VALUE,
          headerRenderer: () => (
            <CheckboxTableHeader
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
          key: CHECK_COLUMN
        },
        {
            key: 'icon',
            title: '',
            dataKey: 'icon',
            cellRenderer: iconRenderer,
            align: Column.Alignment.RIGHT,
            width: 100,
        },
        {
            key: 'label',
            title: 'Package',
            dataKey: 'label',
            align: Column.Alignment.LEFT,
            width: 200,
        }
      ];
    }

    onSelectAllChange(clearAll) {
      if (clearAll) {
        this.props.clearCheckedProjects();
      } else {
        const projects = this.props.projectList.projects ? this.props.projectList.projects.map(project => project.id) : [];
        this.props.setCheckedProjects(projects);
      }
    }

    onSelectChange(checked, rowData) {
      const projects = cloneArray(this.props.checkedProjects);
      const id = rowData.id;
      const index = projects.indexOf(id);

      if (checked) {
        if (index === -1) {
          projects.push(id);
        }
      } else {
        if (index > -1) {
          projects.splice(index, 1);
        }
      }
      this.props.setCheckedProjects(projects);
    }

    render() {
      let data = [];
      if(this.props.projectList && this.props.projectList.projects) {
        data = this.props.projectList.projects.map((project) => (
          {
            id: project.id,
            icon: 'Archive.svg',
            label: project.label,
          }
        ));
      }

      return (
        <AutoResizer>
          {({ width, height }) => {
                return <BaseTable
                  width={width}
                  height={height}
                  columns={this.projectListColumns}
                  data={data}
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
    projectList: store.projectList,
    checkedProjects: checkedProjects(store)
  };
}, { setCheckedProjects, clearCheckedProjects })(CheckboxTable);
