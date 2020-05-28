import React, { Component } from 'react';
import BaseTable, { AutoResizer, Column } from 'react-base-table'
import 'react-base-table/styles.css';
import { downloadFile } from '../actions/downloadActions';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt=''/>
    </div>
  )

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

export default class Downloads extends Component {
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
