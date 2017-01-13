import { Patterns } from '../helpers/pattern';

export function complexity(password: string) {
    return Patterns.Password.test(password);
};

export default complexity;