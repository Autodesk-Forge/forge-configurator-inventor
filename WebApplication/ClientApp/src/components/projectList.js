import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import { Upload24 } from '@hig/icons';
import './projectList.css';

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

    const visible = true; // TBD to be driven by login status
    const uploadContainerClass = visible ? "uploadContainer" : "uploadContainer hidden";

    return (
      <div className="fullheight">
        <div className={uploadContainerClass}>
          <Upload24 className="uploadButton" onClick={ (e) => { console.log(e); }} />
        </div>
        <div className="fullheight">
          <AutoResizer>
            {({ width, height }) => {

                return <BaseTable
                    width={width}
                    height={height}
                    columns={projectListColumns}
                    data={data}
                />;
            }}
          </AutoResizer>
        </div>

      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {
    projectList: store.projectList
  };
})(ProjectList);