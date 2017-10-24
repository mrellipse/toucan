import Vue from 'vue';
import Component from 'vue-class-component';
import './switch.scss';

@Component({
    props: ['enabled', 'value', 'onText', 'offText'],
    template: `<label v-show="show" class="custom-control custom-checkbox">
        <input type="checkbox" class="custom-control-input" 
            :value="value"
            :checked="value"
            @change="onChange($event.target)" />
        <span class="custom-control-indicator"></span> 
        <span class="custom-control-description">{{displayText}}</span>
    </label>`
})
export class Switch extends Vue {

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

    get displayText(): string {
        return this.value ? this.onText : this.offText;
    }
}