export function withTimestamp(data: any) {
    const timestamp = new Date().toISOString();
    return {
        ...data,
        name: `${data.name} ${timestamp}`
    };
}
