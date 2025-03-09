import * as yup from 'yup';

export const GroupTemplateValidation = yup.object({
  name: yup
    .string()
    .required('Name is required'),
  description: yup
    .string()
    .required('Description is required'),
});