import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import { getActiveProject } from '../reducers/mainReducer';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt=''/>
    </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

const typeCellRenderer = function( e ) {
    return <a href={e.rowData.downloadUrl}>{e.cellData}</a>;
};


const columns = [
    {
        key: 'icon',
        title: '',
        dataKey: 'icon',
        cellRenderer: iconRenderer,
        align: Column.Alignment.RIGHT,
        width: 100,
    },
    {
        key: 'type',
        title: 'File Type',
        dataKey: 'type',
        cellRenderer: typeCellRenderer,
        align: Column.Alignment.LEFT,
        width: 150,
    },
    {
        key: 'env',
        title: 'Environment',
        dataKey: 'env',
        align: Column.Alignment.CENTER,
        width: 200,
    }
];

export class Downloads extends Component {
    render() {

        const data = [
            {
                id: 'updatedIam',
                icon: 'products-and-services-24.svg',
                type: 'IAM',
                downloadUrl: this.props.activeProject.modelDownloadUrl,
                env: 'Model',
                clickHandler: (e) => {
                    console.log('IAM');
                    console.log(e);
                }
            },
            {
                id: 'rfa',
                icon: 'products-and-services-24.svg',
                type: 'RFA',
                downloadUrl: '#',
                env: 'Model',
                clickHandler: (e) => {
                    console.log('RFA');
                    console.log(e);
                }
            }
        ];

        return <AutoResizer>
            {({ width, height }) => {
                // reduce size by 16 (twice the default border of tabContent)
                const newWidth = width-16;
                const newHeight = height-16;
                return <BaseTable
                    width={newWidth}
                    height={newHeight}
                    columns={columns}
                    data={data}
                    rowEventHandlers={{
                        onClick: (e) => { e.rowData.clickHandler(e); }
                    }}
                />;
            }}
        </AutoResizer>;
    }
}

export default connect(function(store) {
    const activeProject = getActiveProject(store);
    return {
        activeProject: activeProject
    };
})(Downloads);