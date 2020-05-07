const actionTypes = {
   DISMISS_UPDATEMSG: 'DISMISS_UPDATEMSG'
}

export default actionTypes;

export const dismissUpdateMessage = () => {
   return {
       type: actionTypes.DISMISS_UPDATEMSG
   };
};
