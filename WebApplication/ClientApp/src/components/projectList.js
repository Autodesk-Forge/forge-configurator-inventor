import React, { Component } from 'react';

/** Dummy component for Project page stub */
export default class ProjectList extends Component {
  render() { return (<div style={{ textAlign: 'center', paddingTop: '30px', color: '#777' }}><h3>The page is not yet implemented<br/>Please switch to the Model tab</h3></div>); }
}

//import {connect} from 'react-redux';
//import PropTypes from 'prop-types';

// /** Dummy class to display project list */
// class ProjectList extends Component {

//       const projects = this.props.projectList.projects;
//       const infos = this.props.notifications;

//       if (! projects) {

//         return (<span>No projects loaded</span>);
//       } else {

//         return (
//           <div>
//             <ul>
//               {
//                 projects.map((project) => (<li key={project.id}>{project.label}</li>))
//               }
//             </ul>
//             {
//               infos.map((info, index) => (<div key={index}>{info}</div>))
//             }
//           </div>
//         );
//       }
//     }
// }

// ProjectList.propTypes = {
//   projectList: PropTypes.object,
//   notifications: PropTypes.array
// };

// export default connect(function (store) {
//   return {
//     projectList: store.projectList,
//     notifications: store.notifications
//   };
// })(ProjectList);