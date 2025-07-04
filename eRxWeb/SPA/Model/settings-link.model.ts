export class SettingsLinkModel {
    Label: string;
    ActionUrl: number;
    LaunchType: LinkLaunchType;
}

export enum LinkLaunchType {
    InPlace = 0,
    NewWindow = 1,
    Modal = 2
}