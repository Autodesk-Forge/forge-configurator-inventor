/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

const Autodesk = window.Autodesk;

function ForgePdfViewExtension(viewer, options) {
    Autodesk.Viewing.Extension.call(this, viewer, options);

    this.subToolbar = null;
    this.prevbutton = new Autodesk.Viewing.UI.Button('drawing-button-prev');
    this.nextbutton = new Autodesk.Viewing.UI.Button('drawing-button-next');
}

ForgePdfViewExtension.prototype = Object.create(Autodesk.Viewing.Extension.prototype);
ForgePdfViewExtension.prototype.constructor = ForgePdfViewExtension;

ForgePdfViewExtension.prototype.load = function () {

    if (this.viewer.toolbar) {
        // Toolbar is already available, create the UI
        this.createUI();
    } else {
        // Toolbar hasn't been created yet, wait until we get notification of its creation
        this.onToolbarCreatedBinded = this.onToolbarCreated.bind(this);
        this.viewer.addEventListener(Autodesk.Viewing.TOOLBAR_CREATED_EVENT, this.onToolbarCreatedBinded);
    }

    this.onModelAdded = this.onModelAdded.bind(this);
    this.switchToPage = this.switchToPage.bind(this);
    // listen when added model (switched sheet page)
    this.viewer.addEventListener(Autodesk.Viewing.MODEL_ADDED_EVENT, this.onModelAdded);

    return true;
};

ForgePdfViewExtension.prototype.onModelAdded = function (event) {
    // setup prev/next buttons by selected model
    this.setupButtonsVisibility(event.model);
};

ForgePdfViewExtension.prototype.onToolbarCreated = function () {
    this.viewer.removeEventListener(Autodesk.Viewing.TOOLBAR_CREATED_EVENT, this.onToolbarCreatedBinded);
    this.onToolbarCreatedBinded = null;
    this.createUI();
};

ForgePdfViewExtension.prototype.createUI = function() {

    if (this.subToolbar != null)
        return;

    const thisExtension = this;

    this.prevbutton.onClick = function () {
        const jumpTo = thisExtension.getActivePage(thisExtension.viewer.model) - 1;
        thisExtension.switchToPage(thisExtension.viewer, jumpTo);
    };

    this.prevbutton.addClass('drawing-button-prev');
    this.prevbutton.setIcon('drawing-icon-prev');
    this.prevbutton.setToolTip('Previous Drawing Sheet');

    this.nextbutton.onClick = function () {
        const jumpTo = thisExtension.getActivePage(thisExtension.viewer.model) + 1;
        thisExtension.switchToPage(thisExtension.viewer, jumpTo);
    };

    this.nextbutton.addClass('drawing-button-next');
    this.nextbutton.setIcon('drawing-icon-next');
    this.nextbutton.setToolTip('Next Drawing Sheet');

    // SubToolbar
    this.subToolbar = new Autodesk.Viewing.UI.ControlGroup('custom-drawing-toolbar');
    this.subToolbar.addControl(this.prevbutton);
    this.subToolbar.addControl(this.nextbutton);

    this.viewer.toolbar.addControl(this.subToolbar);

    this.setupButtonsVisibility(thisExtension.viewer.model);
};

ForgePdfViewExtension.prototype.getActivePage = function (model) {
    let actualPage = 1; // default
    if(model && model.getDocumentNode()) {
        // read actual page
        actualPage = model.getDocumentNode().data.page;
    }
    return actualPage;
};

ForgePdfViewExtension.prototype.switchToPage = function(viewer, pageToShow) {
    const model = viewer.model;
    if(model && model.getDocumentNode()) {
        const rootNode = model.getDocumentNode().getRootNode();
        const bubbleNode = rootNode.children[pageToShow - 1];
        viewer.loadDocumentNode(rootNode.getDocument(), bubbleNode);
    }
};

ForgePdfViewExtension.prototype.setupButtonsVisibility = function(model) {
    if (this.subToolbar == null)
        return;

    const numPages = model.getData().getPDF().numPages;
    if (numPages === 1) {
        // hide buttons when only one sheet page is available
        this.subToolbar.setVisible(false);
        return;
    }

    const actualPage = this.getActivePage(model);

    // disable prev button on the first page and next on the last one
    const prevState = actualPage === 1 ? Autodesk.Viewing.UI.Button.State.DISABLED : Autodesk.Viewing.UI.Button.State.INACTIVE;
    this.prevbutton.setState(prevState);
    const nextState = actualPage === numPages ? Autodesk.Viewing.UI.Button.State.DISABLED : Autodesk.Viewing.UI.Button.State.INACTIVE;
    this.nextbutton.setState(nextState);

    // show buttons
    this.subToolbar.setVisible(true);
};

ForgePdfViewExtension.prototype.unload = function () {
    if (this.subToolbar) {
        this.viewer.toolbar.removeControl(this.subToolbar);
        this.subToolbar = null;
    }
};


Autodesk.Viewing.theExtensionManager.registerExtension('ForgePdfViewExtension', ForgePdfViewExtension);