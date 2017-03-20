import Vue = require('vue');
import Component from 'vue-class-component';
import './switch.scss';

@Component({
    props: ['enabled', 'value', 'onText', 'offText'],
    template: `<label v-show="show" class="switch switch-slide">
        <input class="switch-input" type="checkbox"
            :value="value"
            :checked="value"
            @change="onChange($event.target)" />
        <span class="switch-label" :data-on="onText" :data-off="offText"></span> 
        <span class="switch-handle"></span>
    </label>`
})
export class SwitchSlide extends Vue {

    enabled: boolean = this.enabled;
    onText: string = this.onText;
    offText: string = this.offText;

    get show() {
        return this.enabled === null || this.enabled === undefined || this.enabled;
    }
    
    value: string = this.value;

    onChange(target: HTMLInputElement) {
        this.$emit('input', target.checked);
    }
}

export default SwitchSlide;