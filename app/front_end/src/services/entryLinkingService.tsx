import { useRef, useState } from "react";
import getCaretCoordinates from 'textarea-caret';

export interface LinkedentryProps {
    id: string;
    name: string;
    fullName: string;
    creationDate: string;
}

export default function useEntryLinking() {

    const [linkedEntries, setLinkedEntries] = useState<LinkedentryProps[]>([]);
    const [showAutoComplete, setShowAutocomplete] = useState<boolean>(false);
    const [autoCompletePosition, setAutoCompletePosition] = useState({ top: 0, left: 0 });
    const [searchText, setSearchText] = useState<string>('');
    const textFieldRef = useRef<HTMLInputElement>(null);
    const textBoxRef = useRef<HTMLInputElement>(null);


    const handleTextChange = (e: React.ChangeEvent<any>, handleChange: (e: React.ChangeEvent<any>) => void) => {
        handleChange(e);

        const text = e.target.value;
        const cursorPosition = e.target.selectionStart || 0;

        const beforeCursor = text.substring(0, cursorPosition);
        const match = beforeCursor.match(/\{\{([^}]*)$/);

        if (match) {
            setSearchText(match[1]);
            setShowAutocomplete(true);

            if(textFieldRef.current) {
                const textField = textFieldRef.current.querySelector('textarea');
                const textBox = textBoxRef.current;
                
                if (textField && textBox) {
                    const cursorCoords = getCaretCoordinates(textField, cursorPosition);
                    const elementCoords = textField?.getBoundingClientRect();
                    const boxElementCords = textBox.getBoundingClientRect();
                    const diffHeight = elementCoords.top - boxElementCords.top;
                    const correctionAmount = cursorCoords.top > elementCoords.height ? elementCoords.height : cursorCoords.top + 20;

                    setAutoCompletePosition({ top: correctionAmount + diffHeight + 5, left: cursorCoords.left });
                }
            }
        } 
        else {
            setShowAutocomplete(false);
        }
    };

    const insertEntryLink = (entry: LinkedentryProps, values: any, setFieldValue: (field: string, value: any) => void) => {
        const text = values.text;
        const cursorPosition = textFieldRef.current?.querySelector('textarea')?.selectionStart || 0;

        const beforeCursor = text.substring(0, cursorPosition);
        const startPos = beforeCursor.lastIndexOf('{{');

        if(startPos !== -1) {
            const newText = text.substring(0, startPos) + `[[${entry.name}]]` + text.substring(cursorPosition);
            setFieldValue('text', newText);

            if(!linkedEntries.some(e => e.id === entry.id)) {
                setLinkedEntries([...linkedEntries, entry]);
            }

            setShowAutocomplete(false);
        }
    };

    const extractLinkedEntries = (text: string, allEntries: LinkedentryProps[]) => {
        const matches = text.match(/\[\[(.*?)\]\]/g) || [];
        const names = matches.map(match => match.slice(2, -2));
        return allEntries.filter(e => names.includes(e.name));
    };

    const removeEntryLink = (entry: LinkedentryProps, values: any, setFieldValue: (field: string, value: any) => void) => {
        const linkPattern = new RegExp(`\\[\\[${entry.name}\\]\\]`, 'g');
        const newText = values.text.replace(linkPattern, '');
        setFieldValue('text', newText);
        setLinkedEntries(linkedEntries.filter(e => e.id !== entry.id));
    };

    const initializeLinks = (text: string, allEntries: LinkedentryProps[]) => {
        const extractedEntries = extractLinkedEntries(text, allEntries);
        setLinkedEntries(extractedEntries);
        return extractedEntries;
    };

    const resetLinking = () => {
        setLinkedEntries([]);
        setShowAutocomplete(false);
        setSearchText('');
    }

    return {
        linkedEntries,
        setLinkedEntries,
        showAutoComplete,
        autoCompletePosition,
        searchText,
        textFieldRef,
        textBoxRef,
        handleTextChange,
        insertEntryLink,
        extractLinkedEntries,
        removeEntryLink,
        initializeLinks,
        resetLinking,
        setShowAutocomplete
    };
}