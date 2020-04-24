import React, { Component } from 'react';
import {connect} from 'react-redux';  
import ForgeViewer from 'react-forge-viewer';

export class ForgeView extends Component {

    constructor(props){
      super(props);

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
    
    /* Once the viewer has initialized, it will ask us for a forge token.
    We don't need to have a token to access our resouce, but need the viewer component to provide something with... */
    handleTokenRequested(onAccessToken){
        console.log('Token requested by the viewer.');
        if(onAccessToken){
            onAccessToken(null, null);
        }
    }    

    render() {
        var currentHash = this.props.projectList.projects?.find(proj => proj.id === this.props.projectList.activeProjectId)?.currentHash;
        var viewableUrn = currentHash ? `/data/${currentHash}/svf/bubble.json` : null;

        console.log(`viewableUrn: ${viewableUrn}`);
        
        return (
            <ForgeViewer
                // version="6.0"
                urn={viewableUrn}
                view={this.state.view}
                headless={false}
                onViewerError={() => {}}
                onTokenRequest={this.handleTokenRequested.bind(this)}
                onDocumentLoad={this.handleDocumentLoaded.bind(this)}
                onDocumentError={() => {}}
                onModelLoad={() => {}}
                onModelError={() => {}}
            />            
        )
    }
}

export default connect(function (store){
    return {
      projectList: store.projectList
    }
  })(ForgeView);