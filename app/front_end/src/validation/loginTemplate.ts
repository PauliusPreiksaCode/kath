import * as yup from 'yup';

export const loginTemplateValidation = yup.object({
  email: yup
    .string()
    .email('Email is not valid')
    .required('Email is required'),
  password: yup
    .string()
    .required('Password is required'),
});