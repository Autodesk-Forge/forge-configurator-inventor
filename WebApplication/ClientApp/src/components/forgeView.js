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
        let options = {
            env: 'Local' // , getAccessToken: this.props.onTokenRequest
        };

        Autodesk = window.Autodesk;

        let container = this.viewerDiv.current;
        this.viewer = new Autodesk.Viewing.GuiViewer3D(container);

        Autodesk.Viewing.Initializer(
            options, this.handleViewerInit.bind(this));
      }

    handleViewerInit() {
        var errorCode = this.viewer.start();
        if (errorCode)
            return;

        this.setState({enable:true});

        Autodesk.Viewing.Document.load(
            this.props.activeProject.svf + '/bubble.json', this.onDocumentLoadSuccess.bind(this), () => {}
        );
    }

    onDocumentLoadSuccess(viewerDocument) {
        var defaultModel = viewerDocument.getRoot().getDefaultGeometry();
        this.viewer.loadDocumentNode(viewerDocument, defaultModel);
    }

    render() {
        return (
            <div className="modelContainer fullheight">
                <Message/>
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