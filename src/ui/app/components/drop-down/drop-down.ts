import Vue from 'vue';
import Component from 'vue-class-component';
import { KeyValue } from '../../model';

@Component({
    props: {
        'items': {},
        'multiple': {
            default: false,
            type: Boolean
        },
        'value': {}
    },
    template: `<select :multiple="multiple" @change="onChange">
              <option v-for="pair in keyValues" :value="pair.key">{{pair.value}}</option>
          </select>`
})
export class DropDownSelect extends Vue {

    mounted() {

        let selected: string[] = [];

        if (typeof this.value === 'string') {
            selected = [this.value];
        } else if (Array.isArray(this.value)) {
            selected = this.value.concat([]);
            this.outputArray = true;
        }

        this.inspector((el) => {
            if (selected.find(value => value === el.value && !el.selected))
                el.selected = true;
        });
    }

    inspector(cb: (el: HTMLOptionElement) => void) {
        let root: HTMLSelectElement = <HTMLSelectElement>this.$el;
        for (var i = 0; i < root.children.length; i++) {
            cb(<HTMLOptionElement>root.children[i]);
        }
    }

    onChange(event: Event) {

        let target = <HTMLSelectElement>event.target;

        if (this.outputArray) {
            let values = [];

            this.inspector((el) => {
                if (el.selected)
                    values.push(el.value);
            });

            this.$emit('input', values);
        }
        else
            this.$emit('input', target.value);
    }

    get keyValues(): KeyValue[] {

        if (Array.isArray(this.items)) {

            if (typeof this.items[0] == 'object')
                return this.items;
            else
                return this.items.map((value): KeyValue => {
                    return { key: value, value };
                });
        }

        if (typeof this.items == 'object') {
            return Object.keys(this.items).map((key) => {
                return { key, value: this.items[key] };
            });
        }

        return [];
    }

    items: {} | any[] = this.items;

    multiple: boolean = this.multiple;

    outputArray: boolean = false;

    value: string | string[] = this.value;
}