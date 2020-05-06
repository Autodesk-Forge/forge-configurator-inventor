import React, { Component } from 'react';
import Script from 'react-load-script';
import {connect} from 'react-redux';
import { getActiveProject } from '../reducers/mainReducer';
import './forgeView.css';
import Message from './message';

var Autodesk = null;

export class ForgeView extends Component {

    constructor(props){
      super(props);

      this.viewerDiv = React.createRef();
      this.viewer = null;
      this.state = {
        view:null
      };
    }

    /* after the viewer loads a document, we need to select which viewable to
    display in our component */
    handleDocumentLoaded(doc, viewables){
        if (viewables.length === 0) {
            console.error('Document contains no viewables.');
        } else {
            //Select the first viewable in the list to use in our viewer component
            this.setState({view:viewables[0]});
        }
    }

    handleScriptLoad(){
        const options = {
            env: 'Local' // , getAccessToken: this.props.onTokenRequest
        };

        Autodesk = window.Autodesk;

        const container = this.viewerDiv.current;
        this.viewer = new Autodesk.Viewing.GuiViewer3D(container);
        this.viewer.debugEvents(true);

        Autodesk.Viewing.Initializer(
            options, this.handleViewerInit.bind(this));
      }

    handleViewerInit() {
        const errorCode = this.viewer.start();
        if (errorCode)
            return;

        this.setState({enable:true});

        // orient camera in the same way as it's on the thumbnail
        // corresponding to ViewOrientationTypeEnum.kIsoTopRightViewOrientation
        const viewer = this.viewer;
        this.viewer.addEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT, (event) => {

            const viewCubeExtensionId = "Autodesk.ViewCubeUi";

            // ER: it's not perfect, because the view transition is visible, so this is
            // a place to improve someday
            if (event.extensionId === viewCubeExtensionId) {

                const viewCubeUI = event.target.getExtension(viewCubeExtensionId);
                viewCubeUI.setViewCube("front top right");

                viewer.removeEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT);
            }
        });

        Autodesk.Viewing.Document.load(
            this.props.activeProject.svf + '/bubble.json', this.onDocumentLoadSuccess.bind(this), () => {}
        );
    }

    onDocumentLoadSuccess(viewerDocument) {
        const defaultModel = viewerDocument.getRoot().getDefaultGeometry();
        this.viewer.loadDocumentNode(viewerDocument, defaultModel);
    }

    render() {
        return (
            <div className="modelContainer fullheight">
                <Message className="message"/>
                <div className="viewer" id="ForgeViewer">
                    <div ref={this.viewerDiv}></div>
                    <link rel="stylesheet" type="text/css" href={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/style.css`}/>
                    <Script url={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/viewer3D.js`}
                      onLoad={this.handleScriptLoad.bind(this)}
                    />
                </div>
            </div>
        );
    }
}

export default connect(function (store){
    return {
      activeProject: getActiveProject(store)
    };
  })(ForgeView);