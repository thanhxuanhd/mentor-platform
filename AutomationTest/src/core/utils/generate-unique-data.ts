export function withTimestamp(data: any) {
    const timestamp = new Date().toISOString();
    return {
        ...data,
        name: `${data.name} ${timestamp}`
    };
}

export function withTimestampEmail(data: any) {
    const timestamp = new Date().toISOString().replace(/[-:.TZ]/g, '');
    return {
        ...data,
        email: `${timestamp}${data.email}`
    };
}
