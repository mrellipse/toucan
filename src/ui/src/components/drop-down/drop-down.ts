import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
    props: ['value', 'items'],
    template: `<select @change="onChange($event.target)">
              <option v-for="(value, key) in items" :value="key">{{value}}</option>
          </select>`
})
export class DropDownSelect extends Vue {

    mounted() {

        let root: HTMLSelectElement = <HTMLSelectElement>this.$el;
        let selected: string[] = null;

        if (typeof this.value === 'string') {
            selected = [this.value];
        } else {
            selected = this.value.concat([]);
        }

        for (var i = 0; i < root.children.length; i++) {
            let child: HTMLOptionElement = <HTMLOptionElement>root.children[i];
            if (selected.find(value => value === child.value && !child.selected))
                child.selected = true;
        }
    }

    onChange(target: HTMLSelectElement) {
        
        if (typeof this.value === 'string') {
            this.$emit('input', target.value);
        } else {
            this.$emit('input', [target.value]);
        }        
    }

    items: {} = this.items;
    value: string | string[] = this.value;
}

export default DropDownSelect;