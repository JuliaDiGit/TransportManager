using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TransportManager.DataXML.Serializer
{
    public class XmlSet<TDto> where TDto : class
    {
        private string _path;
        private List<TDto> _dtos;
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(List<TDto>));

        public XmlSet(string path)
        {
            _path = path;
        }

        public List<TDto> Data
        {
            get
            {
                try
                {
                    Load();
                }
                catch (Exception)
                {
                    _dtos = new List<TDto>();
                }
                
                return _dtos;
            }
            set
            {
                _dtos = value;
                Save();
            }
        }

        private void Save()
        {
            TextWriter writer = new StreamWriter(_path);
            _serializer.Serialize(writer, _dtos);
            writer.Close();
        }

        private void Load()
        {
            TextReader reader = new StreamReader(_path);
            _dtos = _serializer.Deserialize(reader) as List<TDto>;
            reader.Close();
        }

    }
}