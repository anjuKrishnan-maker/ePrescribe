var ComponentLoadStatus = {
    Started: 0,
    Stopped: 1,
    Progress: 2
}

class ComponentLoadEventArgs{
    constructor(id: string, status: number) {
        this.id = id; this.loadStatus = status;
    }
    id: string;
    loadStatus:number = ComponentLoadStatus.Started
}
export { ComponentLoadStatus, ComponentLoadEventArgs}
