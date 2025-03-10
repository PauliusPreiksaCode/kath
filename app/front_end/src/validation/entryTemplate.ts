import * as yup from 'yup';

export const EntryTemplateValidation = yup.object({ 
    entryName: yup
        .string()
        .required('Name is required'),
    text: yup
        .string()
        .required('Text is required')
});