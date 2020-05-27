import React, { Component } from 'react';
import BaseTable, { AutoResizer } from 'react-base-table';
import 'react-base-table/styles.css';
import { downloadFile } from '../actions/downloadActions';

const columns = [
    {
        key: 'icon',
        title: '',
        dataKey: 'icon',
        width: 100,
    },
    {
        key: 'type',
        title: 'File Type',
        dataKey: 'type',
        width: 150,
    },
    {
        key: 'env',
        title: 'Environment',
        dataKey: 'env',
        width: 200,
    }
];

const data = [
    {
        id: 'updatedIam',
        icon: null,
        type: 'IAM',
        env: 'Model'
    },
    {
        id: 'rfa',
        icon: null,
        type: 'RFA',
        env: 'Model'
    }
];

const rowEventHandlers = {
    onClick: (e) => { downloadFile(e.rowKey); }
};

export default class Downloads extends Component {
    render() {
        return <AutoResizer>
            {({ width, height }) => (
                <BaseTable
                    width={width}
                    height={height}
                    columns={columns}
                    data={data}
                    rowEventHandlers={rowEventHandlers}
                />
            )}
        </AutoResizer>;
    }
}
