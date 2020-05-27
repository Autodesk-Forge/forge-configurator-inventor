export const downloadFile = (fileType) => {
    switch(fileType) {
        case 'updatedIam': {
            console.log('IAM zip requested');
            return;
        }

        case'rfa': {
            console.log('RFA requested');
            break;
        }

        default:
            return;
    }
};