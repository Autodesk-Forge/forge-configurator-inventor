import repo from '../Repository';
import { updateShowParametersChanged } from './applicationActions';

const actionTypes = {
   DISMISS_UPDATEMSG: 'DISMISS_UPDATEMSG'
};

export default actionTypes;

export const dismissUpdateMessage = () => {
   return {
       type: actionTypes.DISMISS_UPDATEMSG
   };
};

export const hideUpdateMessageBanner = (permanently) => async (dispatch) => {

   if (permanently === true) {
      const result = await repo.sendShowParametersChanged(permanently === false);
      dispatch(updateShowParametersChanged(result));
   }

   dispatch(dismissUpdateMessage());
};
