import { Dropdown, Button } from 'antd';
import { GlobalOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import type { MenuProps } from 'antd';

const LanguageSwitcher = () => {
  const { i18n } = useTranslation();

  const items: MenuProps['items'] = [
    {
      key: 'en',
      label: 'English',
      onClick: () => i18n.changeLanguage('en'),
    },
    {
      key: 'vi',
      label: 'Tiếng Việt',
      onClick: () => i18n.changeLanguage('vi'),
    },
  ];

  const currentLanguage = i18n.language === 'vi' ? 'Tiếng Việt' : 'English';

  return (
    <Dropdown menu={{ items }} placement="bottomRight">
      <Button icon={<GlobalOutlined />}>
        {currentLanguage}
      </Button>
    </Dropdown>
  );
};

export default LanguageSwitcher;