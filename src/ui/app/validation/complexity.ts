import { Patterns } from '../common/pattern';

export function complexity(password: string) {
    return Patterns.Password.test(password);
};