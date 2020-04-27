import React, { Component } from 'react';
import Script from 'react-load-script';
import {connect} from 'react-redux';  
import { getActiveProject } from '../reducers/mainReducer';

var Autodesk = null;
    
/* Once the viewer has initialized, it will ask us for a forge token.
We don't need to have a token to access our resouce, but need the viewer component to provide something with... */
function handleTokenRequested(onAccessToken){
    console.log('Token requested by the viewer.');
    if(onAccessToken){
        onAccessToken(null, null);
    }
}

function onDocumentLoadFailure() {
    console.error('Failed fetching Forge manifest');
}    

function handleViewerError(errorCode){
    console.error('Error loading Forge Viewer. - errorCode:', errorCode);
}  

export class ForgeView extends Component {

    constructor(props){
      super(props);

      this.viewerDiv = React.createRef();
      this.viewer = null;      
      this.state = {
        view:null
      }
    }
    
    /* after the viewer loads a document, we need to select which viewable to
    display in our component */
    handleDocumentLoaded(doc, viewables){
        if (viewables.length === 0) {
        console.error('Document contains no viewables.');
        }
        else{
        //Select the first viewable in the list to use in our viewer component
        this.setState({view:viewables[0]});
        }
    }

    handleScriptLoad(){
        console.log('Autodesk scripts have finished loading.');
    
            let options = {
                env: 'Local' // , getAccessToken: this.props.onTokenRequest
            };
    
            Autodesk = window.Autodesk;
            let container = this.viewerDiv.current;
            this.viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('FV2'));

            Autodesk.Viewing.Initializer(
                options, this.handleViewerInit.bind(this));
      }

    handleViewerInit() {
            console.log('Forge Viewer has finished initializing.');

            console.log('Starting the Forge Viewer...');
            var errorCode = this.viewer.start();
            if (!errorCode){
                    console.log('Forge Viewer has successfully started.');
                    this.setState({enable:true});
    
                    Autodesk.Viewing.Document.load(
                        this.props.activeProject.svf + '/bubble.json', this.onDocumentLoadSuccess.bind(this), onDocumentLoadFailure
                      );
                } else{
              console.error('Error starting Forge Viewer - code:', errorCode);
              handleViewerError(errorCode);
            }
    }


    onDocumentLoadSuccess(viewerDocument) {
        var defaultModel = viewerDocument.getRoot().getDefaultGeometry();
        this.viewer.loadDocumentNode(viewerDocument, defaultModel);
    }    

    render() {
        return (
            <div id="ForgeViewer">
              <div id="FV2" ref={this.viewerDiv}></div>
              <link rel="stylesheet" type="text/css" href={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/style.css`}/>
              <Script url={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/viewer3D.js`}
                onLoad={this.handleScriptLoad.bind(this)}
                onError={handleViewerError}
              />
            </div>            
        )
    }
}

export default connect(function (store){
    return {
      activeProject: getActiveProject(store)
    }
  })(ForgeView);