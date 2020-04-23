    
function handleViewerError(error){
    console.log('Error loading viewer.');
}

/* after the viewer loads a document, we need to select which viewable to
display in our component */
function handleDocumentLoaded(doc, viewables){
    if (viewables.length === 0) {
    console.error('Document contains no viewables.');
    }
    else{
    //Select the first viewable in the list to use in our viewer component
    this.setState({view:viewables[0]});
    }
}

function handleDocumentError(viewer, error){
    console.log('Error loading a document');
}

function handleModelLoaded(viewer, model){
    console.log('Loaded model:', model);
}

function handleModelError(viewer, error){
    console.log('Error loading the model.');
}

/* Once the viewer has initialized, it will ask us for a forge token.
We don't need to have a token to access our resouce, but need the viewer component to provide something with... */
function handleTokenRequested(onAccessToken){
    console.log('Token requested by the viewer.');
    if(onAccessToken){
        onAccessToken(null, null);
    }
}  

const Handlers = {
  handleViewerError,
  handleDocumentLoaded,
  handleDocumentError,
  handleModelLoaded,
  handleModelError,
  handleTokenRequested
};

export default Handlers;