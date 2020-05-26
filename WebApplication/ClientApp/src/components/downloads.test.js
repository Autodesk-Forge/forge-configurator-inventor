import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import Downloads from './downloads';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('Downloads', () => {
    it('Downloads have expected rows', () => {
        let wrapper = mount(<Downloads />);
        expect(wrapper.find('.BaseTable')).toHaveLength(1);
        expect(wrapper.find('.BaseTable__row-cell-text')).toHaveLength(6);
        expect(wrapper.find('.BaseTable__row-cell-text').someWhere(n => n.text() === 'IAM')).toBeTruthy();
        expect(wrapper.containsMatchingElement(
            <div class=".BaseTable__row-cell-text">RFA</div>
        )).toBeTruthy();
    });
  });
});