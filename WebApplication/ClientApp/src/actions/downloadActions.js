import { getActiveProject } from '../reducers/mainReducer';

export const downloadFile = (fileType) => async (dispatch, getState) => {
    const activeProject = getActiveProject(getState());
    let url = null;
    let filename = null;

    switch(fileType) {
        case 'updatedIam': {
            url = activeProject.modelDownloadUrl;
            filename = 'model.zip';
            break;
        }

        case'rfa': {
            url = activeProject.rfaDownloadUrl;
            filename = 'rfa.zip';
            break;
        }

        default:
            break;
    }

    if(url) {
        const a = document.createElement('a');
        a.href = url;
        a.download = filename; // set the file name
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click(); //this is probably the key - simulatating a click on a download link

        // would be nicer, but not working for me yet
        // also needs
        //   "permissions": [ "downloads" ]
        // in manifest.json, which is not added there yet
        /*
        const downloading = window.browser.downloads.download({
            url
        });

        downloading.then(() => {}, () => { console.log('Error downloading file'); });
        */
    }
};