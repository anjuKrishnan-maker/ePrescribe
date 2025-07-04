export class MessageModel {
    Message: string;
    Icon: MessageIcon;
    Tag: string;
    ShowCloseButton: boolean;

    public constructor(message: string, icon: MessageIcon, tag: string, showclosebutton: boolean )
    {
        this.Message = message;
        this.Tag = tag;
        this.Icon = icon;
        this.ShowCloseButton = showclosebutton;
    }
}

export enum MessageIcon {
    Success = 0,
    Error,
    Information
}