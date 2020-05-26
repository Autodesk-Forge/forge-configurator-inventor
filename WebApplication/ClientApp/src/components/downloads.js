import React, { Component } from 'react';
import BaseTable, { AutoResizer } from 'react-base-table'
import 'react-base-table/styles.css'

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
        id: '1',
        icon: null,
        type: 'IAM',
        env: 'Model'
    },
    {
        id: '2',
        icon: null,
        type: 'RFA',
        env: 'Model'
    }
];

export default class Downloads extends Component {
    render () {
        return <AutoResizer>
            {({ width, height }) => (
                <BaseTable
                    width={width}
                    height={height}
                    columns={columns}
                    data={data}
                />
            )}
        </AutoResizer>
    }
};
