export interface TestCase<T> {
    label: string;
    data: T;
}

export function buildTestCases<T>(items: { [label: string]: T }): TestCase<T>[] {
    return Object.entries(items).map(([label, data]) => ({
        label,
        data
    }));
}
