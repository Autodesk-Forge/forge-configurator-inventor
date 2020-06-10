import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';

const Icon = ({ iconname }) => (
  <div>
    <img src={iconname} alt='' width='16px' height='18px' />
  </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

export const projectListColumns = [
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

export class ProjectList extends Component {
  render() {
    let data = [];
    if(this.props.projectList.projects) {
      data = this.props.projectList.projects.map((project) => (
        {
          id: project.id,
          icon: 'Archive.svg',
          label: project.label,
          clickHandler: () => {}
        }
      ));
    }

    return (
      <AutoResizer>
        {({ width, height }) => {
            // reduce size by 16 (twice the default border of tabContent)
            const newWidth = width-16;
            const newHeight = height-16;
            return <BaseTable
                width={newWidth}
                height={newHeight}
                columns={projectListColumns}
                data={data}
            />;
        }}
      </AutoResizer>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {
    projectList: store.projectList
  };
})(ProjectList);