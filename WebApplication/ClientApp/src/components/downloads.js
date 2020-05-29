import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import { downloadFile } from '../actions/downloadActions';
import { getActiveProject } from '../reducers/mainReducer';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt=''/>
    </div>
  );

const columns = [
    {
        key: 'icon',
        title: '',
        dataKey: 'icon',
        cellRenderer: ({ cellData: iconname }) => <Icon iconname={iconname} />,
        align: Column.Alignment.RIGHT,
        width: 100,
    },
    {
        key: 'type',
        title: 'File Type',
        dataKey: 'type',
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

const data = [
    {
        id: 'updatedIam',
        icon: 'products-and-services-24.svg',
        type: 'IAM',
        env: 'Model'
    },
    {
        id: 'rfa',
        icon: 'products-and-services-24.svg',
        type: 'RFA',
        env: 'Model'
    }
];

const rowEventHandlers = {
    onClick: (e) => { downloadFile(e.rowKey); }
};

export class Downloads extends Component {

    constructor(props) {
        super(props);
    }

    render() {
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
                    rowEventHandlers={rowEventHandlers}
                />;
            }}
        </AutoResizer>;
    }
}

export default connect(function (store){
    return {

        // the project contains `modelDownloadUrl` and (I think)
        // we need to render <a> inside the table, so click on it will start download
        // at least it's recommended solution from https://stackoverflow.com/questions/50694881/how-to-download-file-in-react-js

        activeProject: getActiveProject(store)
    };
  })(Downloads);