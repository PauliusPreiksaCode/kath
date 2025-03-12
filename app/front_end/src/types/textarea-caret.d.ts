declare module 'textarea-caret' {
    export interface CaretPosition {
        top: number;
        left: number;
    }
    export default function getCaretCoordinates(element: HTMLTextAreaElement, position?: number): CaretPosition;
}
