import { assert } from "chai";
import { } from 'mocha';
import Vue from 'vue';
import { dispatchEvent } from './helper';
import { DropDownSelect } from '../../src/ui/app/components/drop-down/drop-down'; // uses webpack alias to resolve when compiling
import { KeyValue } from '../../src/ui/app/model';

const borg: KeyValue = { key: 'borg', value: 'BORG' };
const klingon: KeyValue = { key: 'klingon', value: 'KLINGON' };
const romulan: KeyValue = { key: 'romulan', value: 'ROMULAN' };

Vue.config.productionTip = false;

describe("Component@DropDown", () => {

    function setup(items?: string[], availableItems?: KeyValue[] | {}) {

        let propsData = {
            items: availableItems || [{ key: '', value: null }, borg, klingon, romulan],
            multiple: true,
            value: items || []
        };

        const ctor = Vue.extend(DropDownSelect);
        return new ctor({ propsData: propsData });

    };

    it("creates options from array", () => {
        const vm = setup(null, [borg, klingon]).$mount();

        const target = <HTMLSelectElement>vm.$el;

        assert.equal(target.options.length, 2, vm.$el.outerHTML);
    });

    it("creates options from object", () => {

        let availableItems = {
            borg: 'borg',
            klingon: 'klingon',
            ferengi: 'ferengi',
            romulan: 'romulan'
        };

        const vm = setup(null, availableItems).$mount();

        const target = <HTMLSelectElement>vm.$el;

        assert.equal(target.options.length, 4, target.outerHTML);
    });

    it("sets selected item on mount", () => {

        const vm = setup([klingon.key], null).$mount();

        const target = <HTMLSelectElement>vm.$el;

        assert.equal(target.selectedOptions.length, 1, target.outerHTML);
    });

    it("sets multiple selected items on mount", () => {

        const vm = setup([klingon.key, romulan.key], null).$mount();

        const target = <HTMLSelectElement>vm.$el;

        assert.equal(target.selectedOptions.length, 2, target.outerHTML);
    });


    it("emits 'input' event after selected item changes", (done) => {

        let onInput = (newValue, oldValue) => {
            assert.equal(newValue, romulan.key, newValue);
            done();
        };

        const vm = setup(null, null).$mount();
        vm.$on('input', onInput);

        const target = <HTMLSelectElement>vm.$el;
        target.options[3].selected = true;

        dispatchEvent(target, 'change');
    });
});