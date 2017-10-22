
// attrib : https://github.com/fareez-ahamed/autocomplete-vuejs2/blob/master/src/components/Autocomplete.vue
import * as Vue from 'vue';
import Component from 'vue-class-component';
import { KeyValue } from '../../model'

@Component({
    props: {
        ignoreCase: {
            default: true,
            type: Boolean,
            required: false
        },
        keyValue: {
            type: String,
            required: true
        },
        limit: {
            type: Number,
            default: 10
        },
        suggestions: {
            type: Array,
            required: true
        }
    },
    template: require('./autocomplete.html')
})
export class Autocomplete extends Vue {

    open = false
    current = 0
    caseInsensitive: string = (<any>this).ignoreCase
    initialValue: string = (<any>this).keyValue
    max: number = (<any>this).limit
    value: string = ""
    selected: string = (<any>this).keyValue
    suggestions: KeyValue[]

    get matches() {

        let ignoreCase = this.caseInsensitive || true;
        let regex = new RegExp('(' + this.selected.trim() + ')', ignoreCase ? 'i' : null);

        return this.suggestions.filter(o => regex.test(o.value))
            .slice(0, this.max - 1);
    }

    get openSuggestion() {

        return this.selected && this.open && this.matches.length !== 0;
    }

    updateValue(value) {

        if (!this.open) {
            this.open = true;
            this.current = 0;
        }
    }

    // When enter pressed on the input
    enter() {

        this.$emit('input', this.matches[this.current].key);
        this.open = false;
    }

    // When up pressed while suggestions are open
    up() {
        if (this.current > 0) {
            this.current--;
        }
    }

    // When down is pressed while suggestions are open
    down() {
        if (this.current < this.matches.length - 1) {
            this.current++
        }
    }

    // For highlighting element
    isActive(index) {

        if (this.current === 0) {
            let key = this.matches[index].key;
            return key === this.initialValue;
        }

        return index === this.current;
    }

    // When one of the suggestion is clicked
    suggestionClick(index) {
        this.$emit('input', this.matches[index].key);
        this.open = false;
    }
}