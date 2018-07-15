import Vue from 'vue';
import Component from 'vue-class-component';
import './toggle.scss';

@Component({
    props: ['enabled', 'value', 'onText', 'offText'],
    template: `<label v-show="show" class="form-check-label ml-4">
        <input type="checkbox" class="form-check-input" 
            :value="value"
            :checked="value"
            @change="onChange($event.target)" />
        <span class="custom-control-indicator"></span> 
        <span class="custom-control-description">{{displayText}}</span>
    </label>`
})
export class Toggle extends Vue {

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